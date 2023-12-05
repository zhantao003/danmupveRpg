package test.www

import akka.http.scaladsl.model.HttpRequest
import com.bakka.handler.http.BaseHttpHandler

class TestFanLogin extends BaseHttpHandler{

  override val schema: String = "/test"

  override def process(request: HttpRequest): String = {
    "test ok"
  }
}
