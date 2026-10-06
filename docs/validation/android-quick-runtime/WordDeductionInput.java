import android.os.SystemClock;
import android.view.InputDevice;
import android.view.InputEvent;
import android.view.MotionEvent;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.lang.reflect.Method;
import java.util.ArrayList;

/** External shell-only test tool. Never packaged with the game. */
public final class WordDeductionInput {
  public static void main(String[] args) throws Exception {
    Class<?> managerClass = Class.forName("android.hardware.input.InputManagerGlobal");
    Object manager = managerClass.getMethod("getInstance").invoke(null);
    Method inject = managerClass.getMethod("injectInputEvent", InputEvent.class, int.class);
    ArrayList<Integer> ids = new ArrayList<>();
    ArrayList<Float> xs = new ArrayList<>(), ys = new ArrayList<>();
    BufferedReader input = new BufferedReader(new InputStreamReader(System.in));
    long downTime = 0;
    System.out.println("READY source=TOUCHSCREEN tool=FINGER");
    System.out.flush();
    String line;
    while ((line = input.readLine()) != null) {
      String[] p = line.trim().split(" +");
      if (p[0].equals("quit")) break;
      int action, index = -1;
      if (p[0].equals("cancel")) {
        if (ids.isEmpty()) continue;
        action = MotionEvent.ACTION_CANCEL;
      } else {
        int id = Integer.parseInt(p[1]);
        index = ids.indexOf(id);
        if (p[0].equals("down")) {
          if (index >= 0 || ids.size() >= 2) throw new IllegalArgumentException("invalid contact");
          if (ids.isEmpty()) downTime = SystemClock.uptimeMillis();
          ids.add(id); xs.add(Float.parseFloat(p[2])); ys.add(Float.parseFloat(p[3])); index = ids.size()-1;
          action = index == 0 ? MotionEvent.ACTION_DOWN : MotionEvent.ACTION_POINTER_DOWN | (index << MotionEvent.ACTION_POINTER_INDEX_SHIFT);
        } else {
          if (index < 0) throw new IllegalArgumentException("unknown contact");
          if (p[0].equals("move")) {
            xs.set(index, Float.parseFloat(p[2])); ys.set(index, Float.parseFloat(p[3])); action = MotionEvent.ACTION_MOVE;
          } else if (p[0].equals("up")) {
            action = ids.size() == 1 ? MotionEvent.ACTION_UP : MotionEvent.ACTION_POINTER_UP | (index << MotionEvent.ACTION_POINTER_INDEX_SHIFT);
          } else throw new IllegalArgumentException("unknown command");
        }
      }
      MotionEvent.PointerProperties[] props = new MotionEvent.PointerProperties[ids.size()];
      MotionEvent.PointerCoords[] coords = new MotionEvent.PointerCoords[ids.size()];
      for (int n=0; n<ids.size(); n++) {
        props[n] = new MotionEvent.PointerProperties(); props[n].id=ids.get(n); props[n].toolType=MotionEvent.TOOL_TYPE_FINGER;
        coords[n] = new MotionEvent.PointerCoords(); coords[n].x=xs.get(n); coords[n].y=ys.get(n); coords[n].pressure=1; coords[n].size=0.1f;
      }
      MotionEvent event = MotionEvent.obtain(downTime, SystemClock.uptimeMillis(), action, ids.size(), props, coords, 0, 0, 1, 1, -1, 0, InputDevice.SOURCE_TOUCHSCREEN, 0);
      boolean result = (Boolean)inject.invoke(manager, event, 2);
      event.recycle();
      System.out.println("OK " + line + " injected=" + result); System.out.flush();
      if (p[0].equals("up")) { ids.remove(index); xs.remove(index); ys.remove(index); }
      if (p[0].equals("cancel")) { ids.clear(); xs.clear(); ys.clear(); }
    }
  }
}
